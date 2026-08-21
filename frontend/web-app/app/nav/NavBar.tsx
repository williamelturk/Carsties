import React from 'react'
import { AiOutlineCar } from 'react-icons/ai'
import Search from './Search'
import Logo from './Logo'

export default function NavBar() {
  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      <div className="bg-red-500 text-white py-2 px-4 rounded-md hover:bg-red-600 cursor-pointer">
        Login
      </div>
    </header>
  )
}
