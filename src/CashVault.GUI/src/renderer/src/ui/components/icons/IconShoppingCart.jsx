/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconShoppingCart = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="40"
        height="40"
        viewBox="0 0 40 40"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M18.3333 31.6666C18.3333 33.5075 16.8409 34.9999 15 34.9999C13.159 34.9999 11.6666 33.5075 11.6666 31.6666C11.6666 29.8257 13.159 28.3333 15 28.3333C16.8409 28.3333 18.3333 29.8257 18.3333 31.6666ZM31.6666 31.6666C31.6666 33.5075 30.1742 34.9999 28.3333 34.9999C26.4923 34.9999 25 33.5075 25 31.6666C25 29.8257 26.4923 28.3333 28.3333 28.3333C30.1742 28.3333 31.6666 29.8257 31.6666 31.6666Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M5 6.66661C5 5.74613 5.74619 4.99994 6.66667 4.99994H7.26732C9.65073 4.99994 11.7028 6.68224 12.1702 9.01936L12.3663 9.99994H30.8281C33.5918 9.99994 35.5897 12.6414 34.8375 15.3007L33.1233 21.3608C32.5144 23.5137 30.5494 24.9999 28.3121 24.9999H16.066C13.6826 24.9999 11.6305 23.3176 11.1631 20.9805L8.90162 9.67308C8.74581 8.89404 8.06179 8.33327 7.26732 8.33327H6.66667C5.74619 8.33327 5 7.58708 5 6.66661ZM13.033 13.3333L14.4317 20.3268C14.5875 21.1058 15.2715 21.6666 16.066 21.6666H28.3121C29.0578 21.6666 29.7128 21.1712 29.9158 20.4536L31.63 14.3934C31.7804 13.8616 31.3808 13.3333 30.8281 13.3333H13.033Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconShoppingCart
